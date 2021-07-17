import { Form } from 'react-final-form'
import { useDispatch, useSelector } from 'react-redux'
import styled from 'styled-components/macro'
import { Button } from '../components/Button'
import { Flex } from '../components/FlexBox'
import { InputField } from '../form/InputField'
import { AuthActions } from '../redux/auth'
import { getAuthLoading } from '../redux/auth/selectors'
import { themeColor, themeSpace } from '../theme'

const LoginPageContainer = styled.div`
  display: flex;
  width: 100%;

  justify-content: center;
  align-items: center;
`

const LoginFormContainer = styled.div`
  display: flex;

  border: 1px solid ${themeColor('borderPrimary')};
  border-radius: 4px;
  padding: ${themeSpace(3)};
`

const SubmitButton = styled(Button)`
  margin-top: 16px;
`

export const LoginPage = () => {
  const d = useDispatch()
  const loading = useSelector(getAuthLoading)

  return (
    <LoginPageContainer>
      <LoginFormContainer>
        <Form<{ user: string; password: string }>
          initialValues={{ user: '', password: '' }}
          // eslint-disable-next-line no-console
          onSubmit={async v => {
            try {
              d(AuthActions.signIn({ userId: v.user, password: v.password }))
            } catch (e) {}
          }}
          render={({ handleSubmit }) => (
            <form onSubmit={handleSubmit}>
              <InputField name="user" label="User" placeholder="User" />
              <InputField name="password" label="Password" placeholder="Password" htmlType="password" />

              <Flex justifyContent="center">
                <SubmitButton loading={loading} htmlType="submit" variant="primary">
                  Go
                </SubmitButton>
              </Flex>
            </form>
          )}
        />
      </LoginFormContainer>
    </LoginPageContainer>
  )
}
