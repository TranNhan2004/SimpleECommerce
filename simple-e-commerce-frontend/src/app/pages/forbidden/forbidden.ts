import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Translation } from '../../shared/translation/translation';


@Component({
  selector: 'app-forbidden',
  imports: [RouterLink],
  templateUrl: './forbidden.html',
  styleUrl: './forbidden.css'
})
export class Forbidden extends Translation {

}
