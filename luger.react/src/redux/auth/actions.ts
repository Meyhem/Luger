import { ActionType, createAction } from 'typesafe-actions'
import { createLoadingActions } from '../loading'

export const AuthActions = {
  setToken: createAction('AUTH/SET_TOKEN')<{
    token: string
  }>(),
  signIn: createAction('AUTH/SIGN_IN')<{ userId: string }>(),
  logout: createAction('AUTH/LOGOUT')(),
  ...createLoadingActions('AUTH')
}

export type AuthActions = ActionType<typeof AuthActions>
