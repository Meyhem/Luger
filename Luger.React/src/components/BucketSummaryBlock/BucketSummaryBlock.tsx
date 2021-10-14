import styled from 'styled-components/macro'
import { BucketSummary } from '../../redux/summary'

type BucketSummaryBlockProps = {
  bucket: string
  summary: BucketSummary
}

const Container = styled.div`
  width: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  border: 1px solid #aaaaaa;
  margin: 6px;
`

const Heading = styled.div`
  width: 100%;
  display: flex;
  justify-content: center;

  font-size: 24px;
  font-weight: bold;
`

export const BucketSummaryBlock = ({ bucket, summary }: BucketSummaryBlockProps) => {
  return (
    <Container>
      <Heading>{bucket}</Heading>
    </Container>
  )
}
