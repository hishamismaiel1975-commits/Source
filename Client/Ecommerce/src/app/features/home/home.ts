import { Component } from '@angular/core';
import { Products } from '../../shared/products/products';

@Component({
  imports: [Products],
  selector: 'app-home',
  styleUrl: './home.css',
  templateUrl: './home.html',
})
export class Home {
  onSearch(event: SubmitEvent) {
    event.preventDefault();

    console.log('Search');
  }
}
