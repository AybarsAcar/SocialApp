import { observer } from 'mobx-react-lite';
import { toast } from 'react-toastify';
import { Button, Header, Icon, Segment } from 'semantic-ui-react';
import agent from '../../app/api/agent';
import useQuery from '../../app/common/util/hooks';

function RegisterSuccess() {
  // get the email from the query string
  const email = useQuery().get('email') as string;

  function handleConfirmEmailAddress() {
    agent.Account.resendEmailConfirm(email)
      .then(() => {
        toast.success('Verification email sent - please check your email');
      })
      .catch((error) => {
        console.log(error);
      });
  }

  return (
    <Segment placeholder textAlign="center">
      <Header icon color="green">
        <Icon name="check" />
        Successfully registered!
      </Header>

      <p>
        Pleace check your email (including junk email) for the verification
        email
      </p>

      {email && (
        <>
          <p>Didn't receive the email? Click the button below to resend</p>
          <Button
            primary
            onClick={handleConfirmEmailAddress}
            content="Resend email"
            size="huge"
          />
        </>
      )}
    </Segment>
  );
}

export default observer(RegisterSuccess);
