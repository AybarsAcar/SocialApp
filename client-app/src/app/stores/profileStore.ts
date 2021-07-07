import { makeAutoObservable, runInAction } from 'mobx';
import agent from '../api/agent';
import { Profile } from '../models/profile';
import { store } from './store';

/**
 * Profile state cached in our client
 */
export default class ProfileStore {
  profile: Profile | null = null;
  loadingProfile = false;

  constructor() {
    makeAutoObservable(this);
  }

  /**
   * returns if the profile displayed belongs to the currently logged in user
   */
  get isCurrentUser() {
    if (store.userStore.user && this.profile) {
      return store.userStore.user.username === this.profile.username;
    }
    return false;
  }

  loadProfile = async (username: string) => {
    this.loadingProfile = true;

    try {
      const profile = await agent.Profiles.get(username);
      runInAction(() => {
        this.profile = profile;
        this.loadingProfile = false;
      });
    } catch (error) {
      console.log(error);
      runInAction(() => {
        this.loadingProfile = false;
      });
    }
  };
}
