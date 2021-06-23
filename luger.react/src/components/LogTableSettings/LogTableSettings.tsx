import { Field, Form, FormSpy } from 'react-final-form'
import { useDispatch, useSelector } from 'react-redux'
import styled from 'styled-components/macro'
import { SearchActions } from '../../redux/search'
import { selectSettings } from '../../redux/search/selectors'
import { RootState } from '../../redux/types'
import { themeColor } from '../../theme'
import { Checkbox } from '../Checkbox'
import { Flex } from '../FlexBox'
import { FormControl } from '../FormControl'

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
}

export const LogTableSettings = ({ bucket }: LogTableSettingsProps) => {
  const d = useDispatch()
  const settings = useSelector((state: RootState) => selectSettings(state, bucket))

  return (
    <LogTableSettingsContainer>
      <Form<TableSettingsType>
        initialValues={settings}
        onSubmit={values => {
          d(SearchActions.setSettings({ bucket: bucket, settings: values }))
        }}
        render={({ handleSubmit, form }) => {
          return (
            <form onSubmit={handleSubmit}>
              <FormSpy onChange={form.submit} subscription={{ values: true }} />
              <Flex flexWrap="wrap">
                <Section width="50%">
                  <Field
                    name="wide"
                    type="checkbox"
                    render={p => <FormControl {...p} component={Checkbox} htmlType="checkbox" />}
                    componentChildren="Wide table"
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
