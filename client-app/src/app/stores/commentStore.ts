import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from '@microsoft/signalr';
import { makeAutoObservable, runInAction } from 'mobx';
import { ChatComment } from '../models/comment';
import { store } from './store';

export default class CommentStore {
  comments: ChatComment[] = [];

  hubConnection: HubConnection | null = null;

  constructor() {
    makeAutoObservable(this);
  }

  createHubConnection = (activityId: string) => {
    // make sure there is selected activity in activity store
    if (store.activityStore.selectedActivity) {
      // create the connection
      this.hubConnection = new HubConnectionBuilder()
        .withUrl('http://localhost:5000/chat?activityId=' + activityId, {
          accessTokenFactory: () => store.userStore.user!.token,
        })
        .withAutomaticReconnect()
        .configureLogging(LogLevel.Information)
        .build();

      // start connection
      this.hubConnection
        .start()
        .catch((error) =>
          console.log('Error establishing the connection: ' + error)
        );

      // call the method in our API
      this.hubConnection.on('LoadComments', (comments: ChatComment[]) => {
        // update the observable
        runInAction(() => {
          // format the date string to JavaScript Date Object
          // append Z to make it UTC Time
          comments.forEach((comment) => {
            comment.createdAt = new Date(comment.createdAt + 'Z');
          });
          this.comments = comments;
        });
      });

      // on a new comment is receives
      this.hubConnection.on('ReceiveComment', (comment: ChatComment) => {
        // add the comment to the state
        runInAction(() => {
          // format the date string to JavaScript Date Object
          comment.createdAt = new Date(comment.createdAt);

          this.comments.unshift(comment);
        });
      });
    }
  };

  stopHubConnection = () => {
    this.hubConnection
      ?.stop()
      .catch((error) => console.log('Error stopping connection: ' + error));
  };

  clearComments = () => {
    this.comments = [];
    this.stopHubConnection();
  };

  addComments = async (values: any) => {
    values.activityId = store.activityStore.selectedActivity?.id;

    try {
      // invoke a method in the server
      await this.hubConnection?.invoke('SendComment', values);
    } catch (error) {
      console.log(error);
    }
  };
}
