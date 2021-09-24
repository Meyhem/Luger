import { Menu, Tooltip } from 'antd'
import dayjs, { Dayjs, OpUnitType } from 'dayjs'
import arrayMutators from 'final-form-arrays'
import _ from 'lodash'
import { useCallback } from 'react'
import { Field, Form, FormSpy } from 'react-final-form'
import { FieldArray } from 'react-final-form-arrays'
import { useDispatch, useSelector } from 'react-redux'
import styled from 'styled-components/macro'
import { forceNumber } from '../../form/utils'
import { LabelFilter, LogLevel, SearchActions } from '../../redux/search'
import { selectFilter, selectSettings } from '../../redux/search/selectors'
import { RootState } from '../../redux/types'
import { themeColor } from '../../theme'
import { Button } from '../Button'
import { DatePicker } from '../DatePicker'
import { Dropdown } from '../Dropdown'
import { Box, Flex } from '../FlexBox'
import { FormControl } from '../FormControl'
import { Input } from '../Input'
import { Select } from '../Select'
import { Text } from '../Text'
import { LabelFilterField } from './LabelFilterField'

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

type LocalLabelFilter = { id: string } & LabelFilter

type FilterType = {
  levels: LogLevel[]
  from: Dayjs
  to: Dayjs
  page: number
  pageSize: number
  message: string
  labels: LocalLabelFilter[]
}

export const LogFilter = ({ bucket }: LogFilterProps) => {
  const d = useDispatch()
  const filter = useSelector((state: RootState) => selectFilter(state, bucket))
  const settings = useSelector((state: RootState) => selectSettings(state, bucket))
  // eslint-disable-next-line react-hooks/exhaustive-deps
  const debouncedSubmit = useCallback(
    _.debounce((values: FilterType) => {
      d(
        SearchActions.setFilter({
          bucket: bucket,
          filter: {
            levels: values.levels,
            from: values.from.toJSON(),
            to: values.to.toJSON(),
            page: values.page || 0,
            pageSize: values.pageSize || 50,
            message: values.message,
            labels: _.map(values.labels, l => ({ name: l.name, value: l.value }))
          }
        })
      )
    }, Math.max(settings.autoSubmitDelay ?? 1500, 200)),
    [settings.autoSubmitDelay]
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
          message: filter.message || '',
          labels: _.map(filter.labels, l => ({ ...l, id: _.uniqueId('labelId_') }))
        }}
        mutators={{ ...arrayMutators }}
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
          // console.log(values)
          return (
            <form onSubmit={handleSubmit}>
              <FormSpy
                onChange={state => {
                  if (_.some(_.values(state.dirtyFields))) {
                    form.submit()
                  }
                }}
                subscription={{ active: true, dirtyFields: true, values: true }}
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

                <Section width="50%">
                  <Field
                    name="message"
                    render={p => <FormControl {...p} label="Message (regex)" component={Input} size="middle" />}
                  />
                </Section>

                <Section width="50%" alignItems="center">
                  <Box paddingTop={26}>
                    <Dropdown
                      trigger={['click']}
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
                  </Box>
                </Section>

                <Section width="70%" flexDirection="column" alignItems="flex-start">
                  <Text color="primary" bold>
                    Labels
                  </Text>
                  <FieldArray name="labels">
                    {({ fields }) => (
                      <Flex flexDirection="column">
                        {_.map(fields.value, (f, i) => (
                          <LabelFilterField
                            key={f.id}
                            id={f.id}
                            // eslint-disable-next-line lodash/matches-shorthand
                            onDelete={id => fields.remove(_.findIndex(values.labels, l => l.id === id))}
                            name={`labels.${i}`}
                          />
                        ))}
                      </Flex>
                    )}
                  </FieldArray>
                  {_.every(values.labels, l => !!l.name) && (
                    <Button
                      padding="0"
                      onClick={() => form.mutators.push('labels', { id: _.uniqueId('labelId_'), name: '', value: '' })}
                    >
                      Add label filter +
                    </Button>
                  )}
                </Section>

                <Section width="30%" alignItems="flex-end">
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
