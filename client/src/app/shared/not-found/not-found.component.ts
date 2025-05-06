import {Component, inject, OnInit} from '@angular/core';
import {CommonModule} from "@angular/common";
import {MatButtonModule} from "@angular/material/button";
import {MatIconModule} from "@angular/material/icon";
import {MatCardModule} from "@angular/material/card";
import {ActivatedRoute, Router} from "@angular/router";

@Component({
  selector: 'app-not-found',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule, MatCardModule],
  templateUrl: './not-found.component.html',
  styleUrl: './not-found.component.scss'
})
export class NotFoundComponent implements OnInit {

  urlDigitada = '';
  private route = inject(ActivatedRoute);
  private router = inject(Router);


  ngOnInit(): void {

    this.urlDigitada = this.router.url;
  }

}
