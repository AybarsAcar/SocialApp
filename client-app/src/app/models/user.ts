/**
 * user object on the client
 */
export interface User {
  username: string;
  displayName: string;
  token: string;
  image?: string;
}

/**
 * required user information to login and register
 */
export interface UserFormValues {
  email: string;
  password: string;
  displayName?: string;
  username?: string;
}
