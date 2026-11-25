import { Link } from 'react-router-dom'
import { useAuth } from '@/auth/useAuth'
import { Button, buttonVariants } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'

export default function HomePage() {
  const { user, isLoading, logout } = useAuth()

  return (
    <main className="mx-auto max-w-xl p-6">
      <Card>
        <CardHeader>
          <CardTitle>CleanMatch</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <p>מצאי מנקה מתאימה לפי מרחק, זמינות ודירוג.</p>
          {isLoading ? null : user ? (
            <div className="space-y-3">
              <p>שלום, {user.userName} ({user.role})</p>
              {user.role === 'customer' && (
                <div className="flex gap-2">
                  <Link to="/chat" className={buttonVariants()}>חיפוש בצ׳אט</Link>
                  <Link to="/search" className={buttonVariants({ variant: 'outline' })}>חיפוש בטופס</Link>
                </div>
              )}
              <Button variant="outline" onClick={logout}>התנתקי</Button>
            </div>
          ) : (
            <div className="flex gap-2">
              <Link to="/login" className={buttonVariants()}>התחברות</Link>
              <Link to="/signup" className={buttonVariants({ variant: 'outline' })}>הרשמה</Link>
            </div>
          )}
        </CardContent>
      </Card>
    </main>
  )
}
