const pad = (n: number) => String(n).padStart(2, '0')

// "09:00" + 2.5 hours -> "11:30:00". Returns null when the booking would pass midnight (the API requires the same day).
export function addHours(start: string, hours: number): string | null {
  const [h, m] = start.split(':').map(Number)
  const total = h * 60 + m + Math.round(hours * 60)
  if (total >= 24 * 60) return null
  return `${pad(Math.floor(total / 60))}:${pad(total % 60)}:00`
}

// "2026-11-25" -> "25.11.2026"
export function formatDate(isoDate: string): string {
  return new Date(`${isoDate.slice(0, 10)}T00:00:00`).toLocaleDateString('he-IL')
}

// "09:00:00" -> "09:00"
export const hhmm = (time: string) => time.slice(0, 5)
