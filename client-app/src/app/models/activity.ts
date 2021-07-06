import { act } from 'react-dom/test-utils';
import { ObjectSchema } from 'yup';
import { Profile } from './profile';

export interface Activity {
  id: string;
  title: string;
  date: Date | null;
  description: string;
  category: string;
  city: string;
  venue: string;

  hostUsername?: string;

  isCancelled?: boolean;

  isGoing?: boolean; // whether a user is going

  isHost?: boolean; // wheter the logged in user is the host

  host?: Profile;

  attendees?: Profile[];
}

export class Activity implements Activity {
  constructor(init?: ActivityFormValues) {
    Object.assign(this, init); // populates all of the properties that it can
  }
}

export class ActivityFormValues {
  id?: string = undefined;
  title: string = '';
  category: string = '';
  description: string = '';
  date: Date | null = null;
  city: string = '';
  venue: string = '';

  constructor(activity?: ActivityFormValues) {
    if (activity) {
      this.id = activity.id;
      this.title = activity.title;
      this.category = activity.category;
      this.description = activity.description;
      this.date = activity.date;
      this.venue = activity.venue;
      this.city = activity.city;
    }
  }
}
