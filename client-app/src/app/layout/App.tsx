import { Container } from 'semantic-ui-react';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import NavBar from './NavBar';
import { observer } from 'mobx-react-lite';
import { Route, Switch, useLocation } from 'react-router';
import HomePage from '../../features/home/HomePage';
import ActivityForm from '../../features/activities/form/ActivityForm';
import ActivityDetails from '../../features/activities/details/ActivityDetails';
import TestErrors from '../../features/errors/TestError';
import { ToastContainer } from 'react-toastify';
import NotFound from '../../features/errors/NotFound';
import ServerError from '../../features/errors/ServerError';
import { useStore } from '../stores/store';
import { useEffect } from 'react';
import LoadingComponent from './LoadingComponent';
import ModalContainer from '../common/modals/ModalContainer';
import ProfilePage from '../../features/profiles/ProfilePage';
import PrivateRoute from './PrivateRoute';
import RegisterSuccess from '../../features/users/RegisterSuccess';
import ConfirmEmail from '../../features/users/ConfirmEmail';

function App() {
  // get the location so we can re-render the component as the route change
  const location = useLocation();

  // get the user info to persist logged in state
  const { commonStore, userStore } = useStore();

  useEffect(() => {
    if (commonStore.token) {
      userStore.getUser().finally(() => commonStore.setApploaded());
    } else {
      // check for facebook accesstoken
      userStore.getFacebookLoginStatus().then(() => commonStore.setApploaded());
    }
  }, [commonStore, userStore]);

  if (!commonStore.appLoaded)
    return <LoadingComponent content="Loading app..." />;

  return (
    <>
      <ToastContainer position="bottom-right" hideProgressBar />

      <ModalContainer />

      <Route exact path="/" component={HomePage} />

      <Route
        path={'/(.+)'}
        render={() => (
          <>
            <NavBar />
            <Container style={{ marginTop: '7em' }}>
              <Switch>
                <PrivateRoute
                  exact
                  path="/activities"
                  component={ActivityDashboard}
                />
                <PrivateRoute
                  exact
                  key={location.key}
                  path={['/createActivity', '/manage/:id']}
                  component={ActivityForm}
                />

                <PrivateRoute
                  exact
                  path="/activities/:id"
                  component={ActivityDetails}
                />

                <PrivateRoute
                  exact
                  path="/profiles/:username"
                  component={ProfilePage}
                />

                <Route
                  exact
                  path="/account/registerSuccess"
                  component={RegisterSuccess}
                />

                <Route
                  exact
                  path="/account/verifyEmail"
                  component={ConfirmEmail}
                />

                {/* Remove the following routes in build */}
                <Route exact path="/errors" component={TestErrors} />
                <Route exact path="/server-error" component={ServerError} />

                {/* NOT FOUND PAGE - This is a fallback page when the route is not found */}
                <Route component={NotFound} />
              </Switch>
            </Container>
          </>
        )}
      />
    </>
  );
}

export default observer(App);
