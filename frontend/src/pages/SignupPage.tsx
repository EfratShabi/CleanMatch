import { useState } from 'react'
import { Link } from 'react-router-dom'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'
import { CleanerSignupForm } from '@/pages/signup/CleanerSignupForm'
import { CustomerSignupForm } from '@/pages/signup/CustomerSignupForm'

type Kind = 'customer' | 'cleaner'

export default function SignupPage() {
  const [kind, setKind] = useState<Kind>('customer')

  return (
    <main className="mx-auto max-w-md p-6">
      <Card>
        <CardHeader>
          <CardTitle>הרשמה</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <div className="flex gap-2">
            <Button
              type="button"
              className="flex-1"
              variant={kind === 'customer' ? 'default' : 'outline'}
              onClick={() => setKind('customer')}
            >
              אני לקוחה
            </Button>
            <Button
              type="button"
              className="flex-1"
              variant={kind === 'cleaner' ? 'default' : 'outline'}
              onClick={() => setKind('cleaner')}
            >
              אני מנקה
            </Button>
          </div>
          {kind === 'customer' ? <CustomerSignupForm /> : <CleanerSignupForm />}
          <p className="text-sm">
            כבר יש לך חשבון? <Link to="/login" className="underline">להתחברות</Link>
          </p>
        </CardContent>
      </Card>
    </main>
  )
}
