import _ from 'lodash'
import { FC, useCallback } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import { generatePath, Link, useLocation } from 'react-router-dom'
import styled from 'styled-components/macro'
import { AuthActions } from '../../redux/auth'
import { getBuckets } from '../../redux/auth/selectors'
import { themeColor } from '../../theme'
import { Routes } from '../../utils/routes'
import { Button } from '../Button'
import { Flex } from '../FlexBox'
import { Text } from '../Text'

type LayoutProps = {
  heading?: string
}

const LayoutContainer = styled.div`
  display: flex;
  flex: 1;
  max-width: 100vw;
`

const Sider = styled.div`
  position: fixed;
  top: 0;
  left: 0;

  display: flex;
  flex-direction: column;

  background: ${themeColor('bgSecondary')};
  width: 20vw;
  height: 100%;
`

const SiderItem = styled.div<{ active?: boolean; clickable?: boolean }>`
  padding: 16px 32px;
  font-weight: ${({ active }) => (active ? 'bold' : 'normal')};

  cursor: ${({ clickable }) => (clickable ? 'pointer' : 'normal')};
  user-select: none;

  &:hover {
    background: ${themeColor('bgPrimary')};
  }
`

const SiderItemGroup = styled.div`
  display: flex;
  flex-direction: column;
  border-top: 1px solid ${themeColor('borderSecondary')};
  border-bottom: 1px solid ${themeColor('borderSecondary')};
`

const Content = styled.div`
  width: 100%;
  max-width: (80vw - 30px); // 30px compensate scroll bar
  padding: 16px 32px 16px calc(20vw + 32px);
`

export const Layout: FC<LayoutProps> = ({ children, heading }) => {
  const buckets = useSelector(getBuckets)
  const { pathname } = useLocation()
  const d = useDispatch()

  const logout = useCallback(() => d(AuthActions.logout()), [d])

  return (
    <LayoutContainer>
      <Sider>
        <SiderItem>
          <Flex justifyContent="space-between">
            <Link to={Routes.Dashboard}>
              <Text fontSize="24px">Luger</Text>
            </Link>

            <Button variant="transparent" onClick={logout}>
              <Text fontSize="24px">⍇</Text>
            </Button>
          </Flex>
        </SiderItem>

        <SiderItemGroup>
          {_.map(buckets, b => {
            const bucketPath = generatePath(Routes.Bucket, { bucket: b })
            const isActiveItem = pathname === bucketPath
            return (
              <Link key={b} to={bucketPath}>
                <SiderItem active={isActiveItem} clickable>
                  {isActiveItem && <>&raquo;</>} Bucket/{b}
                </SiderItem>
              </Link>
            )
          })}
        </SiderItemGroup>
      </Sider>
      <Content>
        {heading && <h1>{heading}</h1>}
        {children}
      </Content>
    </LayoutContainer>
  )
}
