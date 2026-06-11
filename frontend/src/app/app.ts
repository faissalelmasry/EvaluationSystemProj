import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { JsonPipe } from '@angular/common';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [JsonPipe], // Needed to display the JSON result
  template: `
    <div style="padding: 2rem; font-family: sans-serif;">
      <h1>Backend Connection Test 🚀</h1>
      
      @if (data()) {
        <div style="background: #f4f4f4; padding: 1rem; border-radius: 8px;">
          <h3>Data from .NET API:</h3>
          <pre>{{ data() | json }}</pre>
        </div>
      } @else {
        <p>Loading data from backend...</p>
      }
    </div>
  `
})
export class AppComponent implements OnInit {
  private http = inject(HttpClient);
  
  // Using an Angular Signal to hold our API response
  data = signal<any>(null);

  ngOnInit() {
    // The proxy will automatically route this to https://localhost:<your-port>/api/department
    // Note: If your endpoint has a different route, change '/api/department' to match it!
    this.http.get('/api/departments').subscribe({
      next: (response) => {
        console.log('Success!', response);
        this.data.set(response);
      },
      error: (err) => {
        console.error('API Error:', err);
        this.data.set({ error: "Failed to connect. Check the console for details." });
      }
    });
  }
}