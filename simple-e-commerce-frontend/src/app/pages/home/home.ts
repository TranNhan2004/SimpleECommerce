import { Component, inject } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { Translation } from '../../shared/translation/translation';

@Component({
  selector: 'app-home',
  imports: [],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home extends Translation {
  protected readonly auth = inject(AuthService);

}
