import jwtDecode from 'jwt-decode'
import _ from 'lodash'
import { createSelector } from 'reselect'
import { isAfterExpiration } from '../../utils/time'
import { createLoadingSelector } from '../loading'
import { RootState } from '../types'

export type TokenPayload = {
  'Luger.Buckets': string
  nameid: string
  exp: number
}

const selectState = (state: RootState) => state.auth

export const getToken = createSelector(selectState, s => s.token)

export const hasToken = createSelector(selectState, s => !!s.token)

export const getDecodedToken = createSelector(getToken, t => {
  try {
    return jwtDecode<TokenPayload>(t)
  } catch (e) {
    return null
  }
})

export const getBuckets = createSelector(getDecodedToken, t => t && _.split(t['Luger.Buckets'], ','))
export const getUserId = createSelector(getDecodedToken, t => t?.nameid)
export const getExpiration = createSelector(getDecodedToken, t => t?.exp)
export const isAuthenticated = createSelector(
  hasToken,
  getExpiration,
  (hasToken, exp) => hasToken && !isAfterExpiration(exp)
)

export const getAuthLoading = createLoadingSelector(selectState)
