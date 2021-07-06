import { makeAutoObservable, reaction } from 'mobx';
import { ServerError } from '../models/serverError';

export default class CommonStore {
  error: ServerError | null = null;
  token: string | null = window.localStorage.getItem('jwt');
  appLoaded = false;

  constructor() {
    makeAutoObservable(this);

    // react to the value changing for logged in
    // this runs when there is a change to this.token or when it initially loads
    reaction(
      () => this.token,
      (token) => {
        if (token) {
          window.localStorage.setItem('jwt', token);
        } else {
          window.localStorage.removeItem('jwt');
        }
      }
    );
  }

  setServerError = (error: ServerError) => {
    this.error = error;
  };

  setToken = (token: string | null) => {
    if (token) {
      this.token = token;
    }
  };

  setApploaded = () => {
    this.appLoaded = true;
  };
}
