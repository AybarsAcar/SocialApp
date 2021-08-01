//
// This file will contain all our requests
// to our Servers
//

import axios, { AxiosError, AxiosResponse } from 'axios';
import { toast } from 'react-toastify';
import { history } from '../..';
import { Activity, ActivityFormValues } from '../models/activity';
import { PaginatedResult } from '../models/pagination';
import { Photo, Profile, UserActivity } from '../models/profile';
import { User, UserFormValues } from '../models/user';
import { store } from '../stores/store';

// add some delay to our requests - for development
const sleep = (delay: number) => {
  return new Promise((resolve) => {
    setTimeout(resolve, delay);
  });
};

// set the default url that axios uses
axios.defaults.baseURL = process.env.REACT_APP_API_URL;

// inteceptor to send our token with requests
axios.interceptors.request.use((config) => {
  const token = store.commonStore.token;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

// set the axios interceptors
// delay all requests to the Server by 1 second
axios.interceptors.response.use(
  async (response) => {
    if (process.env.NODE_ENV === 'development') {
      // add delay for testin in development
      await sleep(1000);
    }

    const pagination = response.headers['pagination'];

    if (pagination) {
      response.data = new PaginatedResult(
        response.data,
        JSON.parse(pagination)
      );

      return response as AxiosResponse<PaginatedResult<any>>;
    }

    return response;
  },
  (error: AxiosError) => {
    const { data, status, config, headers } = error.response!;

    console.log(error.response);

    switch (status) {
      case 400:
        if (typeof data === 'string') {
          // otherwise, other 400 errors so just display the repsonse data from the API
          toast.error(data);
        }

        if (config.method === 'get' && data.errors.hasOwnProperty('id')) {
          // bad guid validation
          history.push('/not-found');
        }

        if (data.errors) {
          // we have the errors object available - form validation error

          const modalStateErrors = [];

          for (const key in data.errors) {
            if (data.errors[key]) {
              modalStateErrors.push(data.errors[key]);
            }
          }
          throw modalStateErrors.flat();
        }

        break;

      case 401:
        if (
          status === 401 &&
          headers['www-authenticate']?.startsWith(
            'Bearer error="invalid_token"'
          )
        ) {
          store.userStore.logout();
          toast.error('Session expired, please login again');
        }
        break;

      case 404:
        history.push('/not-found');
        break;

      case 500:
        store.commonStore.setServerError(data);
        history.push('/server-error');
        break;
    }
    return Promise.reject;
  }
);

const responseBody = <T>(response: AxiosResponse<T>) => response.data;

// this is our requests that we will make to the base URL
// just to make the code reusable
const requests = {
  get: <T>(url: string) => axios.get<T>(url).then(responseBody),
  post: <T>(url: string, body: {}) =>
    axios.post<T>(url, body).then(responseBody),
  put: <T>(url: string, body: {}) => axios.put<T>(url, body).then(responseBody),
  del: <T>(url: string) => axios.delete<T>(url).then(responseBody),
};

const Activities = {
  // list all the activities
  list: (params: URLSearchParams) =>
    axios
      .get<PaginatedResult<Activity[]>>('/activities', { params: params })
      .then(responseBody),

  details: (id: string) => requests.get<Activity>(`/activities/${id}`),

  create: (activity: ActivityFormValues) =>
    requests.post<void>('/activities', activity),

  update: (activity: ActivityFormValues) =>
    requests.put<void>(`/activities/${activity.id}`, activity),

  delete: (id: string) => requests.del<void>(`/activities/${id}`),

  attend: (id: string) => requests.post<void>(`/activities/${id}/attend`, {}),
};

const Account = {
  // gets the current user
  current: () => requests.get<User>('/account'),

  login: (user: UserFormValues) => requests.post<User>('/account/login', user),

  register: (user: UserFormValues) =>
    requests.post<User>('/account/register', user),

  // facebook login
  fbLogin: (accessToken: string) =>
    requests.post<User>(`/account/fbLogin?accessToken=${accessToken}`, {}),

  refreshToken: () => requests.post<User>('/account/refreshToken', {}),

  verifyEmail: (token: string, email: string) =>
    requests.post<void>(
      `/account/verifyEmail?token=${token}&email=${email}`,
      {}
    ),

  resendEmailConfirm: (email: string) =>
    requests.get(`/account/resendEmailConfirmationLink?email=${email}`),
};

// send request to Profiles endpoints
const Profiles = {
  get: (username: string) => requests.get<Profile>(`/profiles/${username}`),
  uploadPhoto: (file: Blob) => {
    let formData = new FormData();
    formData.append('File', file);
    return axios.post<Photo>('photos', formData, {
      headers: { 'Content-type': 'multipart/form-data' },
    });
  },

  setMainPhoto: (id: string) => requests.post(`/photos/${id}/setMain`, {}),

  deletePhoto: (id: string) => requests.del(`/photos/${id}`),

  updateProfile: (profile: Partial<Profile>) =>
    requests.put('/profiles', profile),

  updateFollowing: (username: string) =>
    requests.post(`/follow/${username}`, {}),

  listFollowings: (username: string, predicate: string) =>
    requests.get<Profile[]>(`/follow/${username}?predicate=${predicate}`),

  listActivities: (username: string, predicate: string) =>
    requests.get<UserActivity[]>(
      `/profiles/${username}/activities?predicate=${predicate}`
    ),
};

const agent = {
  Activities,
  Account,
  Profiles,
};

export default agent;
