import { connectRouter } from 'connected-react-router'
import { History } from 'history'
import { combineReducers } from 'redux'
import persistReducer from 'redux-persist/es/persistReducer'
import localStorage from 'redux-persist/lib/storage'
import { authReducer } from './auth'
import { searchReducer } from './search'
import { summaryReducer } from './summary'
// AUTOIMPORT REDUCER
const persistedAuthReducer = persistReducer(
  {
    key: 'user',
    whitelist: ['token'],
    storage: localStorage
  },
  authReducer
)

const persistedSearchReducer = persistReducer(
  {
    key: 'search',
    // whitelist: ['token'],
    storage: localStorage
  },
  searchReducer
)

export const makeRootReducer = (history: History) =>
  combineReducers({
    // AUTOREGISTER REDUCER
    summary: summaryReducer,
    search: persistedSearchReducer,
    router: connectRouter(history),
    auth: persistedAuthReducer
  })
