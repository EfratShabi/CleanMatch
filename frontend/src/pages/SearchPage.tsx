import { useState } from 'react'
import type { FormEvent } from 'react'
import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { searchCleaners } from '@/api/search'
import { FormField } from '@/components/FormField'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { getErrorMessage } from '@/lib/errors'
import { CleanerCard } from '@/pages/search/CleanerCard'
import type { SearchParams } from '@/types/api'

// yyyy-MM-dd in the user's own time zone, used as the earliest date the picker allows
const today = new Date().toLocaleDateString('en-CA')

export default function SearchPage() {
  const [date, setDate] = useState(today)
  const [start, setStart] = useState('09:00')
  const [hours, setHours] = useState('3')
  const [maxPrice, setMaxPrice] = useState('')

  // The query runs only after the form is submitted; typing alone must not call the server.
  const [submitted, setSubmitted] = useState<SearchParams | null>(null)

  const { data, isFetching, error } = useQuery({
    queryKey: ['search', submitted],
    queryFn: ({ signal }) => searchCleaners(submitted!, signal),
    enabled: submitted !== null,
    retry: false,
  })

  function handleSubmit(e: FormEvent) {
    e.preventDefault()
    setSubmitted({
      date,
      start,
      hours: Number(hours),
      maxPrice: maxPrice === '' ? null : Number(maxPrice),
    })
  }

  return (
    <main className="mx-auto max-w-xl space-y-4 p-6">
      <Card>
        <CardHeader>
          <CardTitle>חיפוש מנקה</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <FormField id="date" label="תאריך" type="date" min={today} value={date} onChange={(e) => setDate(e.target.value)} required />
            <FormField id="start" label="שעת התחלה" type="time" value={start} onChange={(e) => setStart(e.target.value)} required />
            <FormField id="hours" label="כמה שעות" type="number" min={0.75} step={0.25} value={hours} onChange={(e) => setHours(e.target.value)} required />
            <FormField id="maxPrice" label="מחיר מקסימלי לשעה (לא חובה)" type="number" min={1} value={maxPrice} onChange={(e) => setMaxPrice(e.target.value)} />
            <Button type="submit" className="w-full" disabled={isFetching}>
              {isFetching ? 'מחפשת...' : 'חפשי'}
            </Button>
          </form>
        </CardContent>
      </Card>

      {error && <p className="text-sm text-destructive">{getErrorMessage(error)}</p>}
      {data && data.length === 0 && <p>לא נמצאו מנקות מתאימות. נסי שעה, יום או מחיר אחרים.</p>}
      {data && data.length > 0 && (
        <section className="space-y-3">
          <h2 className="font-medium">נמצאו {data.length} מנקות</h2>
          {data.map((cleaner) => (
            <CleanerCard key={cleaner.id} cleaner={cleaner} search={submitted!} />
          ))}
        </section>
      )}

      <Link to="/" className="text-sm underline">חזרה לדף הבית</Link>
    </main>
  )
}
