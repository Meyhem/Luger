import { Checkbox as AntCheckbox, CheckboxProps as AntCheckBoxProps } from 'antd'

type CheckboxProps = {} & AntCheckBoxProps

export const Checkbox = ({ children, ...rest }: CheckboxProps) => {
  return <AntCheckbox {...rest}>{children}</AntCheckbox>
}
