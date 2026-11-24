import { useState } from 'react'
import type { ChangeEvent, FormEvent } from 'react'
import { useMutation } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { signupCleaner } from '@/api/accounts'
import { useAuth } from '@/auth/useAuth'
import { FormField } from '@/components/FormField'
import { Button } from '@/components/ui/button'
import { getErrorMessage } from '@/lib/errors'

const emptyForm = {
  userName: '',
  password: '',
  idOfCleaner: '',
  firstName: '',
  lastName: '',
  phonNumber: '',
  city: '',
  street: '',
  mikud: '',
  descriptionOnMe: '',
  pricePerHour: '',
  upToKm: '',
}

export function CleanerSignupForm() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState(emptyForm)
  const [bornIsrael, setBornIsrael] = useState(false)
  const [hebrewSpeaker, setHebrewSpeaker] = useState(true)

  const set = (key: keyof typeof emptyForm) => (e: ChangeEvent<HTMLInputElement>) =>
    setForm({ ...form, [key]: e.target.value })

  const mutation = useMutation({
    mutationFn: async () => {
      await signupCleaner({
        ...form,
        idOfCleaner: Number(form.idOfCleaner),
        pricePerHour: Number(form.pricePerHour),
        upToKm: Number(form.upToKm),
        bornIsrael,
        hebrewSpeaker,
      })
      await login(form.userName, form.password)
    },
    onSuccess: () => navigate('/'),
  })

  function handleSubmit(e: FormEvent) {
    e.preventDefault()
    mutation.mutate()
  }

  return (
    <form onSubmit={handleSubmit} className="space-y-4">
      <FormField id="firstName" label="שם פרטי" value={form.firstName} onChange={set('firstName')} required />
      <FormField id="lastName" label="שם משפחה" value={form.lastName} onChange={set('lastName')} required />
      <FormField id="idOfCleaner" label="תעודת זהות" type="number" value={form.idOfCleaner} onChange={set('idOfCleaner')} required />
      <FormField id="phonNumber" label="טלפון" type="tel" value={form.phonNumber} onChange={set('phonNumber')} required />
      <FormField id="city" label="עיר" value={form.city} onChange={set('city')} required />
      <FormField id="street" label="רחוב" value={form.street} onChange={set('street')} required />
      <FormField id="mikud" label="מיקוד" value={form.mikud} onChange={set('mikud')} required />
      <FormField id="descriptionOnMe" label="קצת עליי" value={form.descriptionOnMe} onChange={set('descriptionOnMe')} />
      <FormField id="pricePerHour" label="מחיר לשעה (בש״ח)" type="number" min={1} value={form.pricePerHour} onChange={set('pricePerHour')} required />
      <FormField id="upToKm" label="מרחק מקסימלי (בק״מ)" type="number" min={1} value={form.upToKm} onChange={set('upToKm')} required />
      <label className="flex items-center gap-2 text-sm">
        <input type="checkbox" checked={bornIsrael} onChange={(e) => setBornIsrael(e.target.checked)} />
        נולדתי בישראל
      </label>
      <label className="flex items-center gap-2 text-sm">
        <input type="checkbox" checked={hebrewSpeaker} onChange={(e) => setHebrewSpeaker(e.target.checked)} />
        אני דוברת עברית
      </label>
      <FormField id="userName" label="שם משתמש" value={form.userName} onChange={set('userName')} autoComplete="username" required />
      <FormField id="password" label="סיסמה" type="password" value={form.password} onChange={set('password')} autoComplete="new-password" required />
      {mutation.isError && <p className="text-sm text-destructive">{getErrorMessage(mutation.error)}</p>}
      <Button type="submit" className="w-full" disabled={mutation.isPending}>
        {mutation.isPending ? 'נרשמת...' : 'הרשמה'}
      </Button>
    </form>
  )
}
