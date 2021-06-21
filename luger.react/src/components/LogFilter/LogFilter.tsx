import { Menu } from 'antd'
import dayjs, { Dayjs, OpUnitType } from 'dayjs'
import _ from 'lodash'
import { Field, Form } from 'react-final-form'
import { useDispatch, useSelector } from 'react-redux'
import styled from 'styled-components/macro'
import { LogLevel, SearchActions } from '../../redux/search'
import { selectFilter } from '../../redux/search/selectors'
import { RootState } from '../../redux/types'
import { themeColor } from '../../theme'
import { Button } from '../Button'
import { DatePicker } from '../DatePicker'
import { Dropdown } from '../Dropdown'
import { Flex } from '../FlexBox'
import { FormControl } from '../FormControl'
import { Input } from '../Input'
import { Select } from '../Select'

type LogFilterProps = {
  bucket: string
}

const LogFilterContainer = styled.div`
  padding: 16px;
  border: 1px solid ${themeColor('borderPrimary')};
  border-radius: 4px;
  margin-bottom: 16px;
`

const Section = styled(Flex)`
  display: flex;
  padding: 8px 16px 0 16px;
`

type FilterType = {
  levels: LogLevel[]
  from: Dayjs
  to: Dayjs
  page: number
  pageSize: number
}

export const LogFilter = ({ bucket }: LogFilterProps) => {
  const d = useDispatch()
  const filter = useSelector((state: RootState) => selectFilter(state, bucket))

  return (
    <LogFilterContainer>
      <Form<FilterType>
        initialValues={{
          levels: filter.levels ?? [],
          from: dayjs.utc(filter.from),
          to: dayjs.utc(filter.to),
          page: filter.page ?? 0,
          pageSize: filter.pageSize ?? 50
        }}
        onSubmit={values => {
          d(
            SearchActions.setFilter({
              bucket: bucket,
              filter: {
                levels: values.levels,
                from: values.from.toJSON(),
                to: values.to.toJSON(),
                page: values.page ?? 0,
                pageSize: values.pageSize ?? 50
              }
            })
          )
        }}
        render={({ handleSubmit, form, values }) => {
          const createJumpHandler = (amount: number, unit: OpUnitType) => () => {
            form.change('from', dayjs().subtract(amount, unit))
            form.change('to', dayjs())
            form.submit()
          }
          return (
            <form onSubmit={handleSubmit}>
              <Flex flexWrap="wrap">
                <Section width="50%">
                  <Field
                    name="levels"
                    render={p => (
                      <FormControl
                        {...p}
                        label="Levels"
                        allowClear={true}
                        component={Select}
                        mode="multiple"
                        options={_.map(_.values(LogLevel), l => ({ label: l, value: l }))}
                      />
                    )}
                  />
                </Section>

                <Section width="25%">
                  <Field
                    name="from"
                    render={p => (
                      <FormControl
                        {...p}
                        label="From"
                        component={DatePicker}
                        showTime={true}
                        size="large"
                        allowClear={false}
                      />
                    )}
                  />
                </Section>
                <Section width="25%">
                  <Field
                    name="to"
                    render={p => (
                      <FormControl
                        {...p}
                        label="To"
                        component={DatePicker}
                        showTime={true}
                        size="large"
                        allowClear={false}
                      />
                    )}
                  />
                </Section>

                <Section width="20%" alignItems="flex-end">
                  <Button onClick={() => form.change('page', Math.max(Number(values.page) - 1, 0))} htmlType="button">
                    &laquo;
                  </Button>
                  <Field
                    name="page"
                    render={p => <FormControl {...p} label="Page" component={Input} size="middle" htmlType="number" />}
                  />
                  <Button onClick={() => form.change('page', Number(values.page) + 1)} htmlType="button">
                    &raquo;
                  </Button>
                </Section>

                <Section width="10%">
                  <Field
                    name="pageSize"
                    render={p => (
                      <FormControl {...p} label="Page size" component={Input} size="middle" htmlType="number" />
                    )}
                  />
                </Section>
                <Section width="20%"></Section>
                <Section width="50%" alignItems="center">
                  <Dropdown
                    overlay={
                      <Menu>
                        <Menu.Item key="1" onClick={createJumpHandler(5, 'minutes')}>
                          5 minutes
                        </Menu.Item>
                        <Menu.Item key="2" onClick={createJumpHandler(15, 'minutes')}>
                          15 minutes
                        </Menu.Item>
                        <Menu.Item key="3" onClick={createJumpHandler(1, 'hour')}>
                          1 hour
                        </Menu.Item>
                        <Menu.Item key="4" onClick={createJumpHandler(3, 'hours')}>
                          3 hours
                        </Menu.Item>
                        <Menu.Item key="5" onClick={createJumpHandler(12, 'hour')}>
                          12 hours
                        </Menu.Item>
                        <Menu.Item key="6" onClick={createJumpHandler(1, 'day')}>
                          1 day
                        </Menu.Item>
                        <Menu.Item key="7" onClick={createJumpHandler(7, 'days')}>
                          7 days
                        </Menu.Item>
                        <Menu.Item key="8" onClick={createJumpHandler(1, 'month')}>
                          1 month
                        </Menu.Item>
                      </Menu>
                    }
                  >
                    <Button variant="primary">Quick ranges ᐯ</Button>
                  </Dropdown>
                </Section>

                <Section width="100%"></Section>

                <Section width="20%" marginTop="16px">
                  <Button htmlType="submit" variant="secondary">
                    Filter &raquo;
                  </Button>
                </Section>
              </Flex>
            </form>
          )
        }}
      />
    </LogFilterContainer>
  )
}
