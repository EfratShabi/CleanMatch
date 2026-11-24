import type { AvailabilitySlot } from '@/types/api'

const DAY_NAMES = ['ראשון', 'שני', 'שלישי', 'רביעי', 'חמישי', 'שישי', 'שבת']

// "08:00:00" -> "08:00"
const hhmm = (time: string) => time.slice(0, 5)

export function AvailabilityList({ slots }: { slots: AvailabilitySlot[] }) {
  if (slots.length === 0) {
    return <p className="text-sm text-muted-foreground">עוד לא הוגדרו שעות עבודה.</p>
  }

  const sorted = [...slots].sort((a, b) => a.dayOfWeek - b.dayOfWeek || a.startTime.localeCompare(b.startTime))

  return (
    <ul className="space-y-1 text-sm">
      {sorted.map((slot) => (
        <li key={slot.id}>
          יום {DAY_NAMES[slot.dayOfWeek]}: {hhmm(slot.startTime)}–{hhmm(slot.endTime)}
        </li>
      ))}
    </ul>
  )
}
