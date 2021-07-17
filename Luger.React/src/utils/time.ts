import dayjs from 'dayjs'

export function isAfterExpiration(exp?: number) {
  return !!exp && dayjs.unix(exp).isBefore(dayjs())
}
