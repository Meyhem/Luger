import { Menu, Tooltip } from 'antd'
import dayjs, { Dayjs, OpUnitType } from 'dayjs'
import _ from 'lodash'
import { useCallback } from 'react'
import { Field, Form, FormSpy } from 'react-final-form'
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

const CenteredInput = styled(Input)`
  text-align: center;
`

const HelpIconTooltip = styled(Tooltip)`
  display: flex;
  align-items: center;
  font-size: 30px;
  color: ${themeColor('primary')};
  cursor: pointer;
  height: 40px;
`

type FilterType = {
  levels: LogLevel[]
  from: Dayjs
  to: Dayjs
  page: number
  pageSize: number
  message: string
}

const forceNumber = (v: any) => (_.isNaN(_.toNumber(v)) ? 0 : _.toNumber(v))

export const LogFilter = ({ bucket }: LogFilterProps) => {
  const d = useDispatch()
  const filter = useSelector((state: RootState) => selectFilter(state, bucket))

  // eslint-disable-next-line react-hooks/exhaustive-deps
  const debouncedSubmit = useCallback(
    _.debounce(
      (values: FilterType) =>
        d(
          SearchActions.setFilter({
            bucket: bucket,
            filter: {
              levels: values.levels,
              from: values.from.toJSON(),
              to: values.to.toJSON(),
              page: values.page || 0,
              pageSize: values.pageSize || 50,
              message: values.message
            }
          })
        ),
      1000,
      { trailing: true }
    ),
    [d]
  )

  return (
    <LogFilterContainer>
      <Form<FilterType>
        initialValues={{
          levels: filter.levels || [],
          from: dayjs.utc(filter.from),
          to: dayjs.utc(filter.to),
          page: filter.page || 0,
          pageSize: filter.pageSize || 50,
          message: filter.message || ''
        }}
        // initialValuesEqual={_.isEqual}
        onSubmit={debouncedSubmit}
        render={({ handleSubmit, form, values }) => {
          const createJumpHandler = (amount: number, unit: OpUnitType) => () => {
            form.change('from', dayjs().subtract(amount, unit))
            form.change('to', dayjs())
            form.submit()
          }
          const createPageNavHandler = (change: number) => () => {
            form.change('page', Math.max(values.page + change, 0))
            form.submit()
          }

          return (
            <form onSubmit={handleSubmit}>
              <FormSpy
                onChange={state => {
                  if (_.some(_.values(state.dirtyFields)) && !state.active) {
                    form.submit()
                  }
                }}
                subscription={{ active: true, dirtyFields: true }}
              />
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

                <Section width="44%">
                  <Field
                    name="message"
                    render={p => <FormControl {...p} label="Message (regex)" component={Input} size="middle" />}
                  />
                </Section>
                <Section width="6%" alignItems="flex-end" fontSize="30px">
                  <HelpIconTooltip
                    placement="bottom"
                    title={
                      <span>
                        Specify regular expression that matches log message
                        <br />
                        Examples:
                        <br />
                        1. Registration failed, correlation id=\d+
                        <br />
                        2. userid=(100|200)
                        <br />
                        3. /cAsE InSeNsItIVe/i
                      </span>
                    }
                  >
                    🛈
                  </HelpIconTooltip>
                </Section>

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

                <Section width="20%" alignItems="flex-end">
                  <Button onClick={createPageNavHandler(-1)} htmlType="button">
                    &laquo;
                  </Button>
                  <Field
                    name="page"
                    format={forceNumber}
                    render={p => (
                      <FormControl
                        {...p}
                        label="Page"
                        component={CenteredInput}
                        size="middle"
                        labelProps={{ textAlign: 'center' }}
                      />
                    )}
                  />
                  <Button onClick={createPageNavHandler(1)} htmlType="button">
                    &raquo;
                  </Button>
                </Section>

                <Section width="10%">
                  <Field
                    name="pageSize"
                    format={forceNumber}
                    render={p => (
                      <FormControl
                        {...p}
                        label="Page size"
                        component={CenteredInput}
                        size="middle"
                        labelProps={{ textAlign: 'center' }}
                      />
                    )}
                  />
                </Section>
              </Flex>
            </form>
          )
        }}
      />
    </LogFilterContainer>
  )
}
