import { makeAutoObservable, runInAction } from 'mobx';
import { history } from '../..';
import agent from '../api/agent';
import { User, UserFormValues } from '../models/user';
import { store } from './store';

export default class UserStore {
  user: User | null = null;

  fbAccessToken: string | null = null;
  fbLoading = false;

  refreshTokenTimeout: any;

  constructor() {
    makeAutoObservable(this);
  }

  get isLoggedIn() {
    return !!this.user;
  }

  login = async (creds: UserFormValues) => {
    try {
      const user = await agent.Account.login(creds);

      // set the token
      store.commonStore.setToken(user.token);

      this.startRefreshTokenTimer(user);

      runInAction(() => {
        this.user = user;
      });

      history.push('/activities');
      store.modalStore.closeModal();
    } catch (error) {
      throw error;
    }
  };

  logout = () => {
    store.commonStore.setToken(null);
    window.localStorage.removeItem('jwt');
    this.user = null;
    history.push('/');
  };

  getUser = async () => {
    try {
      const user = await agent.Account.current();

      store.commonStore.setToken(user.token);

      runInAction(() => {
        this.user = user;
      });

      this.startRefreshTokenTimer(user);
    } catch (error) {
      console.log(error);
    }
  };

  register = async (creds: UserFormValues) => {
    try {
      const user = await agent.Account.register(creds);

      // set the token
      store.commonStore.setToken(user.token);

      this.startRefreshTokenTimer(user);

      runInAction(() => {
        this.user = user;
      });

      history.push('/activities');
      store.modalStore.closeModal();
    } catch (error) {
      throw error;
    }
  };

  setImage = async (image: string) => {
    if (this.user) this.user.image = image;
  };

  setDisplayName = (name: string) => {
    if (this.user) this.user.displayName = name;
  };

  getFacebookLoginStatus = async () => {
    window.FB.getLoginStatus((response) => {
      if (response.status === 'connected') {
        this.fbAccessToken = response.authResponse.accessToken;
      }
    });
  };

  /**
   * facebook login
   * api mehtod called and changes made to our store
   * request to the facebook APIs
   */
  facebookLogin = () => {
    this.fbLoading = true;

    const apiLogin = (accessToken: string) => {
      agent.Account.fbLogin(accessToken)
        .then((user) => {
          store.commonStore.setToken(user.token);

          this.startRefreshTokenTimer(user);

          runInAction(() => {
            this.user = user;
            this.fbLoading = false;
          });

          history.push('/activities');
        })
        .catch((error) => {
          console.log(error);
          runInAction(() => (this.fbLoading = false));
        });
    };

    if (this.fbAccessToken) {
      // if we already have the access token
      apiLogin(this.fbAccessToken);
    } else {
      // get a new accessToken from facebook
      window.FB.login(
        (response) => {
          apiLogin(response.authResponse.accessToken);
        },
        { scope: 'public_profile,email' }
      );
    }
  };

  /**
   * this method is automatically called when the user token is about to expire
   * so the user is logged in throughout their session without distruption
   */
  refreshToken = async () => {
    this.stopRefreshTokenTimer();

    try {
      const user = await agent.Account.refreshToken();

      runInAction(() => (this.user = user));

      store.commonStore.setToken(user.token);

      this.startRefreshTokenTimer(user);
    } catch (error) {
      console.log(error);
    }
  };

  private startRefreshTokenTimer(user: User) {
    const jwtToken = JSON.parse(atob(user.token.split('.')[1]));
    const expires = new Date(jwtToken.exp * 1000);
    const timeout = expires.getTime() - Date.now() - 60 * 1000; // 60 seconds before hte token expires

    this.refreshTokenTimeout = setTimeout(this.refreshToken, timeout);
  }

  private stopRefreshTokenTimer() {
    clearTimeout(this.refreshTokenTimeout);
  }
}
