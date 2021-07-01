import React from 'react'
import { useSelector } from 'react-redux'
import { Redirect, Route, RouteProps } from 'react-router-dom'
import { isAuthenticated } from '../../redux/auth/selectors'
import { Routes } from '../../utils/routes'

type Props = {
  component: React.ElementType
} & RouteProps

export const AuthRoute = ({ component: Component, ...rest }: Props) => {
  const isAuth = useSelector(isAuthenticated)

  return (
    <Route
      {...rest}
      render={props => {
        if (!isAuth) {
          return (
            <Redirect
              to={{
                pathname: Routes.Login
              }}
            />
          )
        }

        return <Component {...props} />
      }}
    />
  )
}
