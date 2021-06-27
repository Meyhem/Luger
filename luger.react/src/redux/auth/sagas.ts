import { notification } from 'antd'
import axios from 'axios'
import { push } from 'connected-react-router'
import { all, put, takeLatest } from 'redux-saga/effects'
import { getType } from 'typesafe-actions'
import { Routes } from '../../utils/routes'
import { AxiosApiResponse } from '../../utils/types'
import { loadingCall } from '../loading'
import { AuthActions } from './actions'

export function* signIn(action: ReturnType<typeof AuthActions.signIn>) {
  try {
    const resp: AxiosApiResponse<any> = yield loadingCall(AuthActions, axios, {
      method: 'post',
      url: '/api/user/token',
      data: { userId: action.payload.userId }
    })

    yield put(AuthActions.setToken({ token: resp.data.data.token }))
    yield put(push({ pathname: Routes.Dashboard }))
  } catch (e) {
    if (axios.isAxiosError(e)) {
      notification.error({ type: 'error', message: 'Login failed', description: e.message })
    }
  }
}

export function* logout() {
  yield put(
    AuthActions.setToken({
      token: ''
    })
  )
}

export function* handleRefreshToken() {
  yield put(AuthActions.setToken({ token: 'refreshed token' }))
}

export function* authSagas() {
  yield all([takeLatest(getType(AuthActions.logout), logout), takeLatest(getType(AuthActions.signIn), signIn)])
}
