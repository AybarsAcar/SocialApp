import { format } from 'date-fns';
import { observer } from 'mobx-react-lite';
import { Link } from 'react-router-dom';
import { Button, Header, Item, Segment, Image, Label } from 'semantic-ui-react';
import { Activity } from '../../../app/models/activity';
import { useStore } from '../../../app/stores/store';

const activityImageStyle = {
  filter: 'brightness(30%)',
};

const activityImageTextStyle = {
  position: 'absolute',
  bottom: '5%',
  left: '5%',
  width: '100%',
  height: 'auto',
  color: 'white',
};

interface Props {
  activity: Activity;
}

function ActivityDetailedHeader({ activity }: Props) {
  const {
    activityStore: { cancelActivityToggle, updateAttendence, loading },
  } = useStore();

  return (
    <Segment.Group>
      <Segment basic attached="top" style={{ padding: '0' }}>
        {activity.isCancelled && (
          <Label
            style={{ position: 'absolute', zIndex: 1000, left: -14, top: 20 }}
            ribbon
            color="red"
            content="Cancelled"
          />
        )}

        <Image
          src={`/assets/categoryImages/${activity.category}.jpg`}
          fluid
          style={activityImageStyle}
        />
        <Segment style={activityImageTextStyle} basic>
          <Item.Group>
            <Item>
              <Item.Content>
                <Header
                  size="huge"
                  content={activity.title}
                  style={{ color: 'white' }}
                />
                <p>{format(activity.date!, 'dd MMM yyyy')}</p>
                <p>
                  Hosted by{' '}
                  <strong>
                    <Link to={`/profiles/${activity.hostUsername}`}>
                      {activity.host?.displayName}
                    </Link>
                  </strong>
                </p>
              </Item.Content>
            </Item>
          </Item.Group>
        </Segment>
      </Segment>
      <Segment clearing attached="bottom">
        {activity.isHost ? (
          <>
            <Button
              onClick={cancelActivityToggle}
              loading={loading}
              disabled={loading}
              color={activity.isCancelled ? 'green' : 'red'}
              floated="left"
              basic
            >
              {activity.isCancelled
                ? 'Re-activate Activity'
                : 'Cancel Activity'}
            </Button>
            <Button
              disabled={activity.isCancelled}
              as={Link}
              to={`/manage/${activity.id}`}
              color="orange"
              floated="right"
            >
              Manage Event
            </Button>
          </>
        ) : activity.isGoing ? (
          <Button
            disabled={loading}
            loading={loading}
            onClick={updateAttendence}
          >
            Cancel attendance
          </Button>
        ) : (
          <Button
            disabled={loading || activity.isCancelled}
            loading={loading}
            onClick={updateAttendence}
            color="teal"
          >
            Join Activity
          </Button>
        )}
      </Segment>
    </Segment.Group>
  );
}

export default observer(ActivityDetailedHeader);