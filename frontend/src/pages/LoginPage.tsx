import { useState } from 'react'
import type { FormEvent } from 'react'
import { useMutation } from '@tanstack/react-query'
import { Link, useNavigate } from 'react-router-dom'
import { useAuth } from '@/auth/useAuth'
import { FormField } from '@/components/FormField'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { ApiError } from '@/lib/api'
import { getErrorMessage } from '@/lib/errors'

export default function LoginPage() {
  const { login } = useAuth()
  const navigate = useNavigate()
  const [userName, setUserName] = useState('')
  const [password, setPassword] = useState('')

  const mutation = useMutation({
    mutationFn: () => login(userName, password),
    onSuccess: () => navigate('/'),
  })

  function handleSubmit(e: FormEvent) {
    e.preventDefault()
    mutation.mutate()
  }

  const error = mutation.error
  const message =
    error instanceof ApiError && error.status === 401
      ? 'שם משתמש או סיסמה שגויים.'
      : getErrorMessage(error)

  return (
    <main className="mx-auto max-w-md p-6">
      <Card>
        <CardHeader>
          <CardTitle>התחברות</CardTitle>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <FormField
              id="userName"
              label="שם משתמש"
              value={userName}
              onChange={(e) => setUserName(e.target.value)}
              autoComplete="username"
              required
            />
            <FormField
              id="password"
              label="סיסמה"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              autoComplete="current-password"
              required
            />
            {mutation.isError && <p className="text-sm text-destructive">{message}</p>}
            <Button type="submit" className="w-full" disabled={mutation.isPending}>
              {mutation.isPending ? 'מתחברת...' : 'התחברי'}
            </Button>
          </form>
          <p className="mt-4 text-sm">
            אין לך חשבון? <Link to="/signup" className="underline">להרשמה</Link>
          </p>
        </CardContent>
      </Card>
    </main>
  )
}
