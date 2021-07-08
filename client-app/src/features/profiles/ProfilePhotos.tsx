import { observer } from 'mobx-react-lite';
import { SyntheticEvent, useState } from 'react';
import { Button, Card, Grid, Header, Image, Tab } from 'semantic-ui-react';
import PhotoUploadWidget from '../../app/common/imageUpload/PhotoUploadWidget';
import { Photo, Profile } from '../../app/models/profile';
import { useStore } from '../../app/stores/store';

interface Props {
  profile: Profile;
}

function ProfilePhotos({ profile }: Props) {
  const {
    profileStore: {
      isCurrentUser,
      uploadPhoto,
      uploading,
      loading,
      setMainPhoto,
      deletePhoto,
    },
  } = useStore();

  const [addPhotoMode, setAddPhotoMode] = useState<boolean>(false);

  const [target, setTarget] = useState('');

  const handleSetAddPhotoMode = () => {
    setAddPhotoMode(!addPhotoMode);
  };

  function handlePhotoUpload(file: Blob) {
    uploadPhoto(file).then(() => setAddPhotoMode(false));
  }

  function handleSetMainPhoto(
    photo: Photo,
    e: SyntheticEvent<HTMLButtonElement>
  ) {
    setTarget(e.currentTarget.name);
    setMainPhoto(photo);
  }

  function handleDeletePhoto(
    photo: Photo,
    e: SyntheticEvent<HTMLButtonElement>
  ) {
    setTarget(e.currentTarget.name);
    deletePhoto(photo);
  }

  return (
    <Tab.Pane>
      <Grid>
        <Grid.Column width={16}>
          <Header floated="left" icon="image" content="Photos" />

          {isCurrentUser && (
            <Button
              onClick={handleSetAddPhotoMode}
              floated="right"
              basic
              content={addPhotoMode ? 'Cancel' : 'Add Photo'}
            />
          )}
        </Grid.Column>
        <Grid.Column width={16}>
          {addPhotoMode ? (
            <PhotoUploadWidget
              uploadPhoto={handlePhotoUpload}
              loading={uploading}
            />
          ) : (
            <Card.Group itemsPerRow={5}>
              {profile.photos?.map((photo) => (
                <Card key={photo.id}>
                  <Image src={photo.url} />

                  {isCurrentUser && (
                    <Button.Group fluid widths={2}>
                      <Button
                        basic
                        color="green"
                        content="Main"
                        name={'main' + photo.id}
                        disabled={photo.isMain || loading}
                        loading={target === 'main' + photo.id && loading}
                        onClick={(e) => handleSetMainPhoto(photo, e)}
                      />
                      <Button
                        onClick={(e) => handleDeletePhoto(photo, e)}
                        loading={target === photo.id && loading}
                        name={photo.id}
                        basic
                        disabled={photo.isMain || loading}
                        color="red"
                        icon="trash"
                      />
                    </Button.Group>
                  )}
                </Card>
              ))}
            </Card.Group>
          )}
        </Grid.Column>
      </Grid>
    </Tab.Pane>
  );
}

export default observer(ProfilePhotos);
