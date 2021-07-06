//
// Main Store to export all the state stores stores
//

import { useContext } from 'react';
import { createContext } from 'react';
import ActivityStore from './activityStore';
import CommonStore from './commonStore';
import { ModalStore } from './modalStore';
import UserStore from './userStore';

interface Store {
  activityStore: ActivityStore;
  commonStore: CommonStore;
  userStore: UserStore;
  modalStore: ModalStore;
}

export const store: Store = {
  activityStore: new ActivityStore(),
  commonStore: new CommonStore(),
  userStore: new UserStore(),
  modalStore: new ModalStore(),
};

export const StoreContext = createContext(store);

// custom react hook to use our stores
export function useStore() {
  return useContext(StoreContext);
}
