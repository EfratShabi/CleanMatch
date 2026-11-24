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

// GET /api/cleaners/search
export type SearchParams = {
  date: string // yyyy-MM-dd
  start: string // HH:mm
  hours: number
  maxPrice: number | null
}

export type CleanerSearchResult = {
  id: number
  firstName: string
  lastName: string
  city: string
  descriptionOnMe: string
  bornIsrael: boolean
  hebrewSpeaker: boolean
  pricePerHour: number
  totalPrice: number
  averageRating: number | null
  reviewCount: number
  distanceKm: number
  matchScore: number
}

// GET /api/cleaners/{id}
export type AvailabilitySlot = {
  id: number
  dayOfWeek: number // 0 = Sunday
  startTime: string // HH:mm:ss
  endTime: string
}

export type CleanerProfile = {
  id: number
  firstName: string
  lastName: string
  phonNumber: string | null // hidden unless there is an active booking with her
  city: string
  street: string | null
  descriptionOnMe: string
  bornIsrael: boolean
  hebrewSpeaker: boolean
  pricePerHour: number
  upToKm: number
  averageRating: number | null
  reviewCount: number
  arrAvailability: AvailabilitySlot[]
}

// GET /api/feedbacks?cleanerId={id}
export type Feedback = {
  id: number
  bookingCode: number
  cleanerId: number
  rating: number
  textOfFeedback: string | null
  createdAt: string
}

// POST /api/bookings (the customer is taken from the token, not sent)
export type BookingCreate = {
  date: string // yyyy-MM-ddT00:00:00
  startWork: string // HH:mm:ss
  endWork: string // HH:mm:ss
  cleanersId: number
}

export type Booking = {
  bookingCode: number
  totalPrice: number
  date: string
  startWork: string
  endWork: string
  customersId: number
  cleanersId: number
  status: string
  cancelledAt: string | null
  cancelledBy: string | null
  isCompleted: boolean
}

// GET /api/auth/me
export type Me = {
  userId: number
  userName: string
  role: Role
  cleanerId: number | null
  customerId: number | null
}
