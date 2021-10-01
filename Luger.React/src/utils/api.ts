import { notification } from 'antd'
import axios, { AxiosInstance, AxiosRequestConfig, AxiosResponse, CancelToken } from 'axios'
import { call, cancelled } from 'redux-saga/effects'

function formatError(e: Error) {
  if (axios.isAxiosError(e)) {
    return `Request failed ${e.message}`
  }
  return `Request failed ${e.toString()}`
}

export const executeApiCall = (axios: AxiosInstance, config: AxiosRequestConfig) => axios(config)

export function createRequestConfiguration(
  config: AxiosRequestConfig,
  { cancelToken }: { cancelToken?: CancelToken } = {}
) {
  const reqData: AxiosRequestConfig = {
    ...config,
    headers: {
      ...config.headers
    },
    cancelToken
  }

  return reqData
}

export function* apiCall<Response>(
  apiCallConfiguration: AxiosRequestConfig
): Generator<any, AxiosResponse<Response>, any> {
  return yield call(api, apiCallConfiguration)
}

export function* api<Response>(apiCallConfiguration: AxiosRequestConfig): Generator<any, AxiosResponse<Response>, any> {
  const source = axios.CancelToken.source()
  try {
    const reqData = createRequestConfiguration(apiCallConfiguration, {
      cancelToken: source.token
    })

    return yield call(executeApiCall, axios, reqData)
  } catch (e: any) {
    notification.error({ type: 'error', message: `Error`, description: formatError(e), duration: 3 })
    throw e
  } finally {
    if (yield cancelled()) {
      source.cancel('cancelled')
    }
  }
}
