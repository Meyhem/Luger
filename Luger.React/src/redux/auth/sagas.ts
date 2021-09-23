import { notification } from 'antd'
import axios, { AxiosResponse } from 'axios'
import { push } from 'connected-react-router'
import dayjs from 'dayjs'
import { all, delay, put, select, takeLatest } from 'redux-saga/effects'
import { getType } from 'typesafe-actions'
import { Routes } from '../../utils/routes'
import { loadingCall } from '../loading'
import { AuthActions } from './actions'
import { getExpiration, hasToken } from './selectors'

export function* signIn(action: ReturnType<typeof AuthActions.signIn>) {
  try {
    yield requestToken(action.payload.userId, action.payload.password)
    yield put(push({ pathname: Routes.Dashboard }))
  } catch (e) {
    if (axios.isAxiosError(e)) {
      notification.error({ type: 'error', message: 'Login failed', description: e.message })
    }
  }
}

export function* watchToken() {
  while (true) {
    yield delay(10000)

    const loggedIn: boolean = yield select(hasToken)
    if (!loggedIn) continue

    const expiration: number = yield select(getExpiration)
    const $expiration = dayjs.unix(expiration)
    const $now = dayjs()
    if ($expiration.subtract(30, 'seconds').isBefore($now)) {
      yield refreshToken()
    }
  }
}

export function* requestToken(userId: string, password: string) {
  const resp: AxiosResponse<any> = yield loadingCall(AuthActions, axios, {
    method: 'post',
    url: '/api/authentication/token',
    data: { userId, password }
  })

  yield put(AuthActions.setToken({ token: resp.data.token }))
}

export function* refreshToken() {
  const resp: AxiosResponse<any> = yield loadingCall(AuthActions, axios, {
    method: 'post',
    url: '/api/authentication/token/refresh'
  })

  yield put(AuthActions.setToken({ token: resp.data.token }))
}

export function* logout() {
  yield put(
    AuthActions.setToken({
      token: ''
    })
  )
  yield put(push({ pathname: Routes.Login }))
}

export function* authSagas() {
  yield all([
    takeLatest(getType(AuthActions.logout), logout),
    takeLatest(getType(AuthActions.signIn), signIn),
    watchToken()
  ])
}
