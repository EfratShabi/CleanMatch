import { Button } from '@/components/ui/button'
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card'

export default function HomePage() {
  return (
    <main className="mx-auto max-w-xl p-6">
      <Card>
        <CardHeader>
          <CardTitle>CleanMatch</CardTitle>
        </CardHeader>
        <CardContent className="space-y-4">
          <p>מצאי מנקה מתאימה לפי מרחק, זמינות ודירוג.</p>
          <Button>התחילי לחפש</Button>
        </CardContent>
      </Card>
    </main>
  )
}
