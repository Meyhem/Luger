import dayjs from 'dayjs'
import _ from 'lodash'
import { FC, useCallback, useState } from 'react'
import { useDispatch, useSelector } from 'react-redux'
import styled, { css } from 'styled-components/macro'
import { LogLevel, SearchActions } from '../../redux/search'
import { selectLogs, selectSettings } from '../../redux/search/selectors'
import { RootState } from '../../redux/types'
import { themeColor } from '../../theme'

const levelColorMap: Record<LogLevel, [string, string]> = {
  Trace: ['#FFFFFF', '#9b9b9b'],
  Debug: ['#FFFFFF', '#9b9b9b'],
  Information: ['#a0a0ff', '#000000'],
  Warning: ['#ff8300', '#000000'],
  Error: ['#ff6363', '#000000'],
  Critical: ['#ff2929', '#000000'],
  None: ['#AAAAAA', '#000000']
}

type LogTableProps = {
  bucket: string
}

const LogTableContainer = styled.table<{ wide: boolean }>`
  table-layout: ${({ wide }) => (wide ? 'auto' : 'fixed')};
  width: ${({ wide }) => (wide ? 'auto' : '100%')};
`

const LogRow = styled.tr`
  margin-bottom: 2px;

  &:hover {
    background: ${themeColor('bgSecondary')};
  }
`

const LogCell = css`
  padding: 8px 16px;
`

const LogCellLevel = styled.td<{ level: LogLevel }>`
  ${LogCell};
  width: 110px;
  vertical-align: top;
  font-weight: bold;
  color: ${({ level }) => levelColorMap[level][1]};
  background: ${({ level }) => levelColorMap[level][0]};
`

const LogCellTime = styled.td`
  ${LogCell};
  width: 150px;
  white-space: nowrap;
  vertical-align: top;
`

const LogCellMessage = styled.td<{ expanded?: boolean }>`
  ${LogCell};
  width: 100%;

  vertical-align: top;
  white-space: ${({ expanded }) => (expanded ? 'normal' : 'nowrap')};

  div {
    overflow: hidden;
    text-overflow: ellipsis;
  }
`

const LogCellLabels = styled.td<{ wide: boolean }>`
  ${LogCell};
  vertical-align: top;
  width: 30%;
  min-width: 30%;
  word-break: ${({ wide }) => (wide ? 'keep-all' : 'break-all')};
`

const Label = styled.div`
  padding: 1px 2px;
  cursor: pointer;
  b {
    color: ${themeColor('primary')};
  }

  &:hover {
    background: ${themeColor('bgPrimary')};
    border-radius: 4px;
  }
`

const LogCellMessageContainer: FC = ({ children }) => {
  const [expanded, setExpanded] = useState(false)
  const toggleExpanded = useCallback(() => setExpanded(!expanded), [expanded])

  return (
    <LogCellMessage expanded={expanded} onDoubleClick={toggleExpanded}>
      {children}
    </LogCellMessage>
  )
}

export const LogTable = ({ bucket }: LogTableProps) => {
  const records = useSelector((state: RootState) => selectLogs(state, bucket))
  const settings = useSelector((state: RootState) => selectSettings(state, bucket))
  const d = useDispatch()

  return (
    <LogTableContainer wide={settings.wide}>
      <tbody>
        {_.map(records, (record, i) => (
          <LogRow key={i}>
            <LogCellLevel level={record.level}>{record.level}</LogCellLevel>
            <LogCellTime>
              <div>
                {dayjs(record.timestamp).utc().format('YYYY-MM-DD')}{' '}
                <b>{dayjs(record.timestamp).utc().format('HH:mm:ss')}</b>
              </div>
            </LogCellTime>
            <LogCellLabels wide={settings.wide}>
              {_.map(record.labels, (v, k) => (
                <Label key={k} onClick={() => d(SearchActions.addLabelFilter({ bucket: bucket, name: k, value: v }))}>
                  <b>{k}</b>: {v}
                </Label>
              ))}
            </LogCellLabels>
            <LogCellMessageContainer>
              {_.map(_.split(record.message, /(\n|\r\n)/), (line, i) => (
                <div key={i}>{line}</div>
              ))}
            </LogCellMessageContainer>
          </LogRow>
        ))}
      </tbody>
    </LogTableContainer>
  )
}
