import generatePicker, { PickerProps } from 'antd/es/date-picker/generatePicker'
import 'antd/es/date-picker/style/index'
import { Dayjs } from 'dayjs'
import dayjsGenerateConfig from 'rc-picker/lib/generate/dayjs'

const AntDatePicker = generatePicker<Dayjs>(dayjsGenerateConfig)

type DatePickerProps = {} & PickerProps<Dayjs>

export const DatePicker = ({ ...props }: DatePickerProps) => {
  return <AntDatePicker {...props} />
}
