import { Component, EventEmitter, Input, Output } from '@angular/core';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-ia-pagination',
  imports: [NgbDropdownModule],
  templateUrl: './ia-pagination.html',
  styleUrl: './ia-pagination.scss'
})
export class IaPagination {
  @Input() totalItemsCount?: number;
  @Input() pageNumber?: number;
  @Input() pageSize?: number;
  @Input() totalPages?: number;

  @Output() pageSizeChanged = new EventEmitter<number>();
  @Output() pageNumberChanged = new EventEmitter<number>();

  pageSizeValues = [5, 10, 30, 50, 100];

  onPageSizeChange(pageSize: number) {
    this.pageSizeChanged.emit(pageSize);
  }

  onPageNumberChange(pageNumber: number): void {
    this.pageNumberChanged.emit(pageNumber);
  }
}
