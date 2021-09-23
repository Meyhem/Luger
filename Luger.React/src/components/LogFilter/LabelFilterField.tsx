import React, { useCallback } from 'react'
import { Field } from 'react-final-form'
import styled from 'styled-components/macro'
import { Button } from '../Button'
import { FormControl } from '../FormControl'
import { Input } from '../Input'

const LabelFilterContainer = styled.div`
  display: flex;
  align-items: flex-start;
  margin-bottom: 8px;
`

export type LabelFilterProps = {
  name: string
  id: string
  onDelete(id: string): void
}

export const LabelFilterField = ({ name, id, onDelete }: LabelFilterProps) => {
  const onDeleteCallback = useCallback(() => onDelete(id), [id, onDelete])

  return (
    <LabelFilterContainer>
      <Field
        // validate={required()}
        name={`${name}.name`}
        render={p => <FormControl {...p} component={Input} placeholder="Label" errorStateIndependent={true} />}
      />
      <Field name={`${name}.value`} render={p => <FormControl {...p} component={Input} placeholder="Value regex" />} />
      <Button onClick={onDeleteCallback}>X</Button>
    </LabelFilterContainer>
  )
}
