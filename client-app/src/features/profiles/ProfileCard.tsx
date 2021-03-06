import { observer } from 'mobx-react-lite';
import { Link } from 'react-router-dom';
import { Card, Image, Icon } from 'semantic-ui-react';
import { Profile } from '../../app/models/profile';
import FollowButton from './FollowButton';

interface Props {
  profile: Profile;
}

function ProfileCard({ profile }: Props) {
  return (
    <Card as={Link} to={`/profiles/${profile.username}`}>
      <Image src={profile.image || '/assets/user.png'} />
      <Card.Content>
        <Card.Header>{profile.displayName}</Card.Header>

        <Card.Description>{profile.bio}</Card.Description>

        <Card.Content extra>
          <Icon name="user" />
          {profile.followerCount}{' '}
          {profile.followerCount > 1 ? 'followers' : 'follower'}
        </Card.Content>
      </Card.Content>

      <FollowButton profile={profile} />
    </Card>
  );
}

export default observer(ProfileCard);
