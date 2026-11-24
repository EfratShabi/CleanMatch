export type Role = 'customer' | 'cleaner' | 'manager'

// POST /api/auth/login
export type LoginResponse = { token: string }

// POST /api/customers
export type CustomerSignup = {
  userName: string
  password: string
  idOfCustomer: number
  firstName: string
  lastName: string
  phonNumber: string
  city: string
  street: string
  mikud: string
  numberFlat: number
}

// POST /api/cleaners
export type CleanerSignup = {
  userName: string
  password: string
  idOfCleaner: number
  firstName: string
  lastName: string
  phonNumber: string
  city: string
  street: string
  mikud: string
  descriptionOnMe: string
  bornIsrael: boolean
  hebrewSpeaker: boolean
  pricePerHour: number
  upToKm: number
}

// GET /api/auth/me
export type Me = {
  userId: number
  userName: string
  role: Role
  cleanerId: number | null
  customerId: number | null
}
