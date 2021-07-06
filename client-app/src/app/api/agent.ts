//
// This file will contain all our requests
//

import axios, { AxiosError, AxiosResponse } from 'axios';
import { toast } from 'react-toastify';
import { history } from '../..';
import { Activity, ActivityFormValues } from '../models/activity';
import { User, UserFormValues } from '../models/user';
import { store } from '../stores/store';

// add some delay to our requests - for development
const sleep = (delay: number) => {
  return new Promise((resolve) => {
    setTimeout(resolve, delay);
  });
};

// set the default url that axios uses
axios.defaults.baseURL = 'http://localhost:5000/api';

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
    await sleep(1000);
    return response;
  },
  (error: AxiosError) => {
    const { data, status, config } = error.response!;

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
        toast.error('Unauthorised');
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
  list: () => requests.get<Activity[]>('/activities'),

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
};

const agent = {
  Activities,
  Account,
};

export default agent;