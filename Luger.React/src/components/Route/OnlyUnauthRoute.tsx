import { useSelector } from 'react-redux'
import { Redirect, Route, RouteProps } from 'react-router-dom'
import { isAuthenticated } from '../../redux/auth/selectors'
import { Routes } from '../../utils/routes'

type Props = RouteProps

export const OnlyUnauthRoute = ({ ...rest }: Props) => {
  const isAuth = useSelector(isAuthenticated)
  return isAuth ? <Redirect to={Routes.Dashboard} /> : <Route {...rest} />
}
