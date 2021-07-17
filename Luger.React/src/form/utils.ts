import _ from 'lodash'

export const forceNumber = (v: any) => (_.isNaN(_.toNumber(v)) ? 0 : _.toNumber(v))
