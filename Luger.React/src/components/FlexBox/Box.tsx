import styled from 'styled-components/macro'
import {
  border,
  BorderProps,
  color,
  ColorProps,
  flexbox,
  FlexboxProps,
  layout,
  LayoutProps,
  space,
  SpaceProps,
  typography,
  TypographyProps
} from 'styled-system'

type StyledProps = SpaceProps & LayoutProps & TypographyProps & ColorProps & FlexboxProps & BorderProps

export type BoxProps = React.PropsWithChildren<
  Omit<StyledProps & JSX.IntrinsicElements['div'], keyof React.ClassAttributes<any>> & {
    as?: React.ElementType
  }
>

export const Box = styled.div<BoxProps>`
  width: 100%;
  box-sizing: 'border-box';
  min-width: 0;

  ${space};
  ${layout};
  ${typography};
  ${color};
  ${flexbox};
  ${border}
`
