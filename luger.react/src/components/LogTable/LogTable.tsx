import dayjs from 'dayjs'
import _ from 'lodash'
import { FC, useCallback, useState } from 'react'
import styled, { css } from 'styled-components/macro'
import { LogLevel, LogRecord } from '../../redux/search'
import { themeColor } from '../../theme'

const levelColorMap: Record<LogLevel, [string, string]> = {
  Debug: ['#FFFFFF', '#9b9b9b'],
  Verbose: ['#FFFFFF', '#9b9b9b'],
  Info: ['#a0a0ff', '#000000'],
  Warning: ['#ff8300', '#000000'],
  Error: ['#ff6363', '#000000'],
  Critical: ['#ff2929', '#000000'],
  Fatal: ['#ff2929', '#000000'],
  Unknown: ['#AAAAAA', '#000000']
}

type LogTableProps = {
  records: LogRecord[]
}

const LogTableContainer = styled.table`
  table-layout: fixed;
  width: 100%;
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
  width: 90px;
  vertical-align: top;
  font-weight: bold;
  white-space: nowrap;
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
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: ${({ expanded }) => (expanded ? 'normal' : 'nowrap')};
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

export const LogTable = ({ records }: LogTableProps) => {
  return (
    <LogTableContainer>
      <tbody>
        {_.map(records, (record, i) => (
          <LogRow key={i}>
            <LogCellLevel level={record.level}>{record.level}</LogCellLevel>
            <LogCellTime>
              <div>
                {dayjs(record.timestamp).format('YYYY-MM-DD')} <b>{dayjs(record.timestamp).format('HH:mm:ss')}</b>
              </div>
            </LogCellTime>
            <LogCellMessageContainer>{record.message}</LogCellMessageContainer>
          </LogRow>
        ))}
      </tbody>
    </LogTableContainer>
  )
}
