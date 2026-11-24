import { useState } from 'react'
import type { ChangeEvent, FormEvent } from 'react'
import { useMutation } from '@tanstack/react-query'
import { useNavigate } from 'react-router-dom'
import { signupCustomer } from '@/api/accounts'
import { useAuth } from '@/auth/useAuth'
import { FormField } from '@/components/FormField'
import { Button } from '@/components/ui/button'
import { getErrorMessage } from '@/lib/errors'

const emptyForm = {
  userName: '',
  password: '',
  idOfCustomer: '',
  firstName: '',
  lastName: '',
  phonNumber: '',
  city: '',
  street: '',
  mikud: '',
  numberFlat: '',
}

export function CustomerSignupForm() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState(emptyForm)

  const set = (key: keyof typeof emptyForm) => (e: ChangeEvent<HTMLInputElement>) =>
    setForm({ ...form, [key]: e.target.value })

  const mutation = useMutation({
    mutationFn: async () => {
      await signupCustomer({
        ...form,
        idOfCustomer: Number(form.idOfCustomer),
        numberFlat: Number(form.numberFlat),
      })
      // Right after signup the user is logged in, so she does not type the password twice.
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
      <FormField id="idOfCustomer" label="תעודת זהות" type="number" value={form.idOfCustomer} onChange={set('idOfCustomer')} required />
      <FormField id="phonNumber" label="טלפון" type="tel" value={form.phonNumber} onChange={set('phonNumber')} required />
      <FormField id="city" label="עיר" value={form.city} onChange={set('city')} required />
      <FormField id="street" label="רחוב" value={form.street} onChange={set('street')} required />
      <FormField id="numberFlat" label="מספר דירה" type="number" value={form.numberFlat} onChange={set('numberFlat')} required />
      <FormField id="mikud" label="מיקוד" value={form.mikud} onChange={set('mikud')} required />
      <FormField id="userName" label="שם משתמש" value={form.userName} onChange={set('userName')} autoComplete="username" required />
      <FormField id="password" label="סיסמה" type="password" value={form.password} onChange={set('password')} autoComplete="new-password" required />
      {mutation.isError && <p className="text-sm text-destructive">{getErrorMessage(mutation.error)}</p>}
      <Button type="submit" className="w-full" disabled={mutation.isPending}>
        {mutation.isPending ? 'נרשמת...' : 'הרשמה'}
      </Button>
    </form>
  )
}
