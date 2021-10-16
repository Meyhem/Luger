import _ from 'lodash'

export function formatApproxTimespan(seconds: number) {
  if (seconds < 1) return '< 1 second'
  if (seconds < 60) return `${_.floor(seconds)} seconds`
  if (seconds < 60 * 60) return `${_.floor(seconds / 60)} minutes`
  if (seconds < 60 * 60 * 24) return `${_.floor(seconds / (60 * 60))} hours`
  return `${_.floor(seconds / (60 * 60 * 24))} days`
}

export function formatApproxByteSize(bytes: number) {
  if (bytes < 2 ** 10) return `${_.round(bytes)} B`
  if (bytes < 2 ** 20) return `${_.round(bytes / 2 ** 10)} KiB`
  if (bytes < 2 ** 30) return `${_.round(bytes / 2 ** 20)} MiB`
  return `${_.round(bytes / 2 ** 30)} GiB`
}
