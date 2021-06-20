import dayjs, { Dayjs } from 'dayjs'
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
import { Flex } from '../FlexBox'
import { FormControl } from '../FormControl'
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
  padding: 0 16px;
`

type FilterType = {
  levels: LogLevel[]
  from: Dayjs
  to: Dayjs
}

export const LogFilter = ({ bucket }: LogFilterProps) => {
  const d = useDispatch()
  const filter = useSelector((state: RootState) => selectFilter(state, bucket))

  return (
    <LogFilterContainer>
      <Form<FilterType>
        initialValues={{
          levels: filter.levels ?? [],
          from: dayjs(filter.from),
          to: dayjs(filter.to)
        }}
        onSubmit={values => {
          d(
            SearchActions.setFilter({
              bucket: bucket,
              filter: {
                levels: values.levels,
                from: values.from.toJSON(),
                to: values.to.toJSON(),
                page: 0,
                pageSize: 20
              }
            })
          )
        }}
        render={({ handleSubmit }) => (
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

              <Section width="20%" marginTop="16px">
                <Button htmlType="submit" variant="secondary">
                  Filter &raquo;
                </Button>
              </Section>
            </Flex>
          </form>
        )}
      />
    </LogFilterContainer>
  )
}
