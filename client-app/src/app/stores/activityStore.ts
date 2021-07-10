//
// Class that stores the state related to activity
//

import { format } from 'date-fns';
import { makeAutoObservable, runInAction } from 'mobx';
import { act } from 'react-dom/test-utils';
import agent from '../api/agent';
import { Activity, ActivityFormValues } from '../models/activity';
import { Pagination, PagingParams } from '../models/pagination';
import { Profile } from '../models/profile';
import { store } from './store';

export default class ActivityStore {
  // class properties are observables
  public activityRegistry = new Map<string, Activity>();

  selectedActivity: Activity | undefined = undefined;
  editMode = false;
  loading = false;
  loadingInitial = false;
  pagination: Pagination | null = null;
  pagingParams = new PagingParams();

  constructor() {
    makeAutoObservable(this);
  }

  setPagingParams = (pagingParams: PagingParams) => {
    this.pagingParams = pagingParams;
  };

  /**
   * create the search params
   */
  get axiosParams() {
    const params = new URLSearchParams();
    params.append('pageNumber', this.pagingParams.pageNumber.toString());
    params.append('pageSize', this.pagingParams.pageSize.toString());

    return params;
  }

  /**
   * computed property
   * returns the activities in an array sorted by date
   */
  get activities() {
    return Array.from(this.activityRegistry.values()).sort(
      (a, b) => a.date!.getTime() - b.date!.getTime()
    );
  }

  /**
   * groups the activities based on their date
   * we use the date of the activities of the key
   * and the value is an array of activities
   */
  get groupedActivities() {
    return Object.entries(
      this.activities.reduce((activities, activity) => {
        const date = format(activity.date!, 'dd MMM yyyy');
        activities[date] = activities[date]
          ? [...activities[date], activity]
          : [activity];
        return activities;
      }, {} as { [key: string]: Activity[] })
    );
  }

  // class methods are actions by default
  loadActivities = async () => {
    this.loadingInitial = true;

    try {
      const result = await agent.Activities.list(this.axiosParams);

      result.data.forEach((activity) => {
        this.setActivity(activity);
      });
      this.setPagination(result.pagination);
      this.setLoadingInitial(false);
    } catch (error) {
      console.log(error);

      this.setLoadingInitial(false);
    }
  };

  setPagination = (pagination: Pagination) => {
    this.pagination = pagination;
  };

  setLoadingInitial = (state: boolean) => {
    this.loadingInitial = state;
  };

  createActivity = async (activity: ActivityFormValues) => {
    // add the user as an attendee and set as host
    const user = store.userStore.user;
    const attendee = new Profile(user!);

    try {
      await agent.Activities.create(activity);

      const newActivity = new Activity(activity);

      newActivity.hostUsername = user!.username;
      newActivity.attendees = [attendee];

      this.setActivity(newActivity);

      runInAction(() => {
        this.selectedActivity = newActivity;
      });
    } catch (error) {
      console.log(error);
    }
  };

  updateActivity = async (activity: ActivityFormValues) => {
    try {
      await agent.Activities.update(activity);

      runInAction(() => {
        if (activity.id) {
          let updatedActivity = { ...this.getActivity(activity.id), ...act };
          this.activityRegistry.set(activity.id, updatedActivity as Activity);
          this.selectedActivity = activity as Activity;
        }
      });
    } catch (error) {
      console.log(error);
    }
  };

  deleteActivity = async (id: string) => {
    this.loading = true;
    try {
      await agent.Activities.delete(id);

      runInAction(() => {
        this.activityRegistry.delete(id);

        this.loading = false;
      });
    } catch (error) {
      console.log(error);

      runInAction(() => {
        this.loading = false;
      });
    }
  };

  /**
   * Method to load an individual activity
   * this method is called when the user refreshed the Activity Detail page
   * when we dont have the activity in memory
   */
  loadActivity = async (id: string) => {
    let activity = this.getActivity(id);

    if (activity) {
      this.selectedActivity = activity;

      return activity;
    } else {
      this.loadingInitial = true;

      try {
        activity = await agent.Activities.details(id);
        this.setActivity(activity);
        this.selectedActivity = activity;
        this.setLoadingInitial(false);

        return activity;
      } catch (error) {
        console.log(error);
        this.setLoadingInitial(false);
      }
    }
  };

  private getActivity = (id: string) => {
    return this.activityRegistry.get(id);
  };

  /**
   * sets the activity as it is loaded
   * @param activity
   */
  private setActivity = (activity: Activity) => {
    // get hold onto the user object - either null if not logged in or the user
    const user = store.userStore.user;

    if (user) {
      // some returns a boolean - true if they are in the attendees list
      activity.isGoing = activity.attendees!.some(
        (a) => a.username === user.username
      );

      // set the activity isHost flag
      activity.isHost = activity.hostUsername === user.username;

      // set hte host
      activity.host = activity.attendees?.find(
        (x) => x.username === activity.hostUsername
      );
    }

    activity.date = new Date(activity.date!);
    this.activityRegistry.set(activity.id, activity);
  };

  /**
   * this will work as a toggle
   */
  updateAttendence = async () => {
    // get the cached in user
    const user = store.userStore.user;

    this.loading = true;
    try {
      await agent.Activities.attend(this.selectedActivity!.id);
      runInAction(() => {
        if (this.selectedActivity?.isGoing) {
          this.selectedActivity.attendees =
            this.selectedActivity.attendees?.filter(
              (a) => a.username !== user?.username
            );
          this.selectedActivity.isGoing = false;
        } else {
          // create a user profile
          const attendee = new Profile(user!);
          this.selectedActivity?.attendees?.push(attendee);
          this.selectedActivity!.isGoing = true;
        }

        this.activityRegistry.set(
          this.selectedActivity!.id,
          this.selectedActivity!
        );
      });
    } catch (error) {
      console.log(error);
    } finally {
      runInAction(() => (this.loading = false));
    }
  };

  cancelActivityToggle = async () => {
    this.loading = true;
    try {
      await agent.Activities.attend(this.selectedActivity!.id);
      runInAction(() => {
        this.selectedActivity!.isCancelled =
          !this.selectedActivity!.isCancelled;

        this.activityRegistry.set(
          this.selectedActivity!.id,
          this.selectedActivity!
        );
      });
    } catch (error) {
      console.log(error);
    } finally {
      runInAction(() => {
        this.loading = false;
      });
    }
  };

  clearSelectedActivity = () => {
    this.selectedActivity = undefined;
  };

  updateAttendeeFollowing = (username: string) => {
    this.activityRegistry.forEach((activity) => {
      activity.attendees?.forEach((attendee) => {
        if (attendee.username === username) {
          attendee.following
            ? attendee.followerCount--
            : attendee.followerCount++;

          attendee.following = !attendee.following;
        }
      });
    });
  };
}
