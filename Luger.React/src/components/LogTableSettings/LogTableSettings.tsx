import _ from 'lodash'
import { Field, Form, FormSpy } from 'react-final-form'
import { useDispatch, useSelector } from 'react-redux'
import styled from 'styled-components/macro'
import { forceNumber } from '../../form/utils'
import { SearchActions } from '../../redux/search'
import { selectSettings } from '../../redux/search/selectors'
import { RootState } from '../../redux/types'
import { themeColor } from '../../theme'
import { Checkbox } from '../Checkbox'
import { Flex } from '../FlexBox'
import { FormControl } from '../FormControl'
import { Input } from '../Input'

type LogTableSettingsProps = {
  bucket: string
}

const LogTableSettingsContainer = styled.div`
  padding: 16px;
  border: 1px solid ${themeColor('borderPrimary')};
  border-radius: 4px;
  margin-bottom: 16px;
`

const Section = styled(Flex)`
  display: flex;
  padding: 8px 16px 0 16px;
`

type TableSettingsType = {
  columns: string[]
  wide: boolean
  autoSubmitDelay: number
}

export const LogTableSettings = ({ bucket }: LogTableSettingsProps) => {
  const d = useDispatch()
  const settings = useSelector((state: RootState) => selectSettings(state, bucket))

  return (
    <LogTableSettingsContainer>
      <Form<TableSettingsType>
        initialValues={settings}
        onSubmit={values => {
          d(
            SearchActions.setSettings({
              bucket: bucket,
              settings: {
                autoSubmitDelay: parseInt(_.toString(values.autoSubmitDelay), 10),
                columns: values.columns,
                wide: values.wide
              }
            })
          )
        }}
        render={({ handleSubmit, form }) => {
          return (
            <form onSubmit={handleSubmit}>
              <FormSpy onChange={form.submit} subscription={{ values: true }} />
              <Flex flexWrap="wrap" alignItems="center">
                <Section width="20%">
                  <Field
                    name="wide"
                    type="checkbox"
                    render={p => <FormControl {...p} component={Checkbox} htmlType="checkbox" />}
                    componentChildren="Wide table"
                  />
                </Section>
                <Section width="30%">
                  <Field
                    name="autoSubmitDelay"
                    type="number"
                    format={forceNumber}
                    render={p => <FormControl label="Auto submit delay (ms)" {...p} component={Input} />}
                  />
                </Section>
              </Flex>
            </form>
          )
        }}
      />
    </LogTableSettingsContainer>
  )
}
